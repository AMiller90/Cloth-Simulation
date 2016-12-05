
namespace Assets.Scripts
{
    using System.Collections.Generic;

    using UnityEngine;

    /// <summary>
    /// Class that simulates the cloth physics.
    /// </summary>
    public class ClothSimulation : MonoBehaviour
    {

        /// <summary>
        /// The drawers.
        /// </summary>
        public List<GameObject> Drawers;

        /// <summary>
        /// The instance.
        /// </summary>
        private static ClothSimulation sInstance;

        /// <summary>
        /// The number of particles.
        /// </summary>
        private readonly int numOfParticles = 36;

        /// <summary>
        /// The gravity.
        /// </summary>
        [SerializeField]
        private float gravity;

        /// <summary>
        /// The ks.
        /// </summary>
        private float ks;

        /// <summary>
        /// The Spring Damper factor.
        /// </summary>
        private float kd;

        /// <summary>
        /// The lo.
        /// </summary>
        private float lo;


        /// <summary>
        /// The prefab.
        /// </summary>
        [SerializeField]
        private GameObject prefab;

        /// <summary>
        /// The prefab damper.
        /// </summary>
        [SerializeField]
        private GameObject prefabDamper;

        /// <summary>
        /// The particles.
        /// </summary>
        private List<Particle> particles;

        /// <summary>
        /// The Mono particles.
        /// </summary>
        private List<MonoParticle> Monoparticles;

        /// <summary>
        /// The springs.
        /// </summary>
        private List<SpringDamper> springs;

        /// <summary>
        /// The triangles.
        /// </summary>
        private List<Triangle> triangles;

        /// <summary>
        /// Gets the self.
        /// </summary>
        public static ClothSimulation Self
        {
            get
            {
                return sInstance;
            }
        }

        /// <summary>
        /// Create the objects for drawing the line renderer.
        /// </summary>
        /// <param name="a_Sd">
        /// The spring damper object.
        /// </param>
        /// <returns>
        /// The <see cref="GameObject"/>.
        /// </returns>
        public GameObject CreateDrawer(SpringDamper a_Sd)
        {
            // Create a gameobject in the scene
            GameObject drawerGo = Instantiate(this.prefabDamper, (a_Sd.P1.Position + a_Sd.P2.Position) / 2f, new Quaternion()) as GameObject;

            // Get the line renderer component
            if (drawerGo != null)
            {
                LineRenderer lr = drawerGo.GetComponent<LineRenderer>();
            
                // Set the color to black
                lr.materials[0].color = Color.black;
            
                // Set the width
                lr.SetWidth(.1f, .1f);
            }

            // Return the object
            return drawerGo;
        }

        /// <summary>
        /// The awake.
        /// </summary>
        private void Awake()
        {// Set singleton instance
            if (sInstance == null)
            {
                sInstance = this;
            }

            this.particles = new List<Particle>();

            this.Monoparticles = new List<MonoParticle>();

            this.triangles = new List<Triangle>();

            this.springs = new List<SpringDamper>();

            this.Drawers = new List<GameObject>();

            // Generate particles
            this.GenerateParticles();

            // Generate the springs
            this.GenerateSprings();

            // Generate the triangles
            this.GenerateTriangles();
        }

        /// <summary>
        /// The start.
        /// </summary>
        private void Start()
        {
            this.gravity = Mathf.Clamp(this.gravity,1, 1);
            this.ks = 0;
            this.kd = 0;
            this.lo = Mathf.Clamp(this.lo, 1, 1);
        }

        /// <summary>
        /// The fixed update.
        /// </summary>
        private void FixedUpdate()
        {
            List<SpringDamper> tempSdList = new List<SpringDamper>();
            List<Triangle> tempTriList = new List<Triangle>();

            // temp list for spring dampers
            foreach (SpringDamper sd in this.springs)  
            {
                tempSdList.Add(sd);
            }

            //temp list for Triangles
            foreach (Triangle t in this.triangles)  
            {
                tempTriList.Add(t);
            }


            // 1.Apply gravity to each particle
            foreach (Particle p in this.particles)
            {
                p.Force = (this.gravity * Vector3.down) * p.Mass;
            }

            // 2.Apply forces to each spring damper
            foreach (SpringDamper sd in tempSdList)
            {
                sd.Lo = this.lo;
                sd.Ks = UiManager.Self.SpringConstant;
                sd.Kd = UiManager.Self.DampingFactor;
                sd.ComputeForces();

                // Check if the break happens
                if (sd.BreakHappens(15) || (sd.P1 == null || sd.P2 == null))
                {
                    Destroy(this.Drawers[this.springs.IndexOf(sd)]);
                    this.Drawers.Remove(this.Drawers[this.springs.IndexOf(sd)]);
                    this.springs.Remove(sd);
                }
            }


            foreach (Triangle t in tempTriList)
            { // Check if there is wind
                if (UiManager.Self.Wind)
                {
                    UiManager.Self.WindResistance = UiManager.Self.WindResistance == 0.0f ? 1 : UiManager.Self.WindResistance;

                    if (!this.springs.Contains(t.Sd1) || !this.springs.Contains(t.Sd2)
                        || !this.springs.Contains(t.Sd3))
                    {
                        this.triangles.Remove(t);
                    }
                    else
                    { // Calculate aerodynamic force on triangle
                        t.CalcAeroForce(Vector3.forward * UiManager.Self.WindResistance);
                    }
                }
            }

            // Apply borders
            this.Borders();

            // Update the particles movement
            foreach (MonoParticle p in this.Monoparticles)
            { // If its not anchored then apply movement
                if (!p.ParticleRef.Anchor)
                {
                   p.transform.position = p.ParticleRef.UpdateParticle();
                }
                else
                {
                    p.ParticleRef.Position = p.transform.position;
                }
            }
        }

        /// <summary>
        /// The late update.
        /// </summary>
        private void LateUpdate()
        {
            List<GameObject> tempSpringDampers = new List<GameObject>();
            foreach (GameObject go in this.Drawers)
            {
                tempSpringDampers.Add(go);
            }
            for (int i = 0; i < tempSpringDampers.Count; i++)
            {
                if (tempSpringDampers[i] != null)
                {
                    LineRenderer lr = tempSpringDampers[i].GetComponent<LineRenderer>();
                    lr.SetPosition(0, this.springs[i].P1.Position);
                    lr.SetPosition(1, this.springs[i].P2.Position);
                }
            }
        }

        /// <summary>
        /// The generate particles.
        /// </summary>
        private void GenerateParticles()
        {
            GameObject holder = new GameObject("Particle Holder");

            float x = 0;
            float y = 0;

            float temp = Mathf.Sqrt(this.numOfParticles);
            float counter = 0;

            // Loop through number of particles
            for (int i = 0; i < this.numOfParticles; i++)
            {
                if (counter == temp)
                {
                    x = 0;
                    y += 2;
                    counter = 0;
                }

                x += 2;

                Particle P = new Particle(new Vector3(x, y, 0));
                // Instantiate and name it
                this.prefab.transform.position = new Vector3(x, y, 0);
                GameObject node = Instantiate(this.prefab) as GameObject;
                node.name = "Particle " + (i + 1);
                node.transform.SetParent(holder.transform);

                // if the instance doesnt have the particle component..then add it
                if (!this.prefab.GetComponent<MonoParticle>())
                {
                    node.AddComponent<MonoParticle>();
                }

                node.GetComponent<MonoParticle>().ParticleRef = P;

                // Add it to the list of particles
                this.particles.Add(P);
                this.Monoparticles.Add(node.GetComponent<MonoParticle>());

                counter++;
            }

            // Set 4 anchors
            this.particles[this.particles.Count - 1].Anchor = true;
            this.particles[this.particles.Count - 6].Anchor = true;
            this.particles[0].Anchor = true;
            this.particles[5].Anchor = true;
        }

        /// <summary>
        /// The generate springs.
        /// </summary>
        private void GenerateSprings()
        {
            GameObject drawObjHolder = new GameObject("LineHolder");
            int width = (int)Mathf.Sqrt(this.numOfParticles);

            foreach (Particle p in this.particles)
            {
                int index = this.FindIndex(this.particles, p);
                p.Neighbors = new List<Particle>();

                // Find and set neighbors
                // Right node
                if ((index + 1) % width > index % width)
                {
                    p.Neighbors.Add(this.particles[index + 1]);
                    SpringDamper sd = new SpringDamper(p, this.particles[index + 1], this.ks, this.kd, this.lo);
                    GameObject obj = this.CreateDrawer(sd);
                    obj.transform.SetParent(drawObjHolder.transform);
                    this.Drawers.Add(obj);
                    this.springs.Add(sd);

                }

                // Node above
                if (index + width < this.particles.Count)
                {
                    p.Neighbors.Add(this.particles[index + width]);
                    SpringDamper sd = new SpringDamper(p, this.particles[index + width], this.ks, this.kd, this.lo);
                    GameObject obj = this.CreateDrawer(sd);
                    obj.transform.SetParent(drawObjHolder.transform);
                    this.Drawers.Add(obj);
                    this.springs.Add(sd);
                }

                // Top left node
                if (index + width - 1 < this.particles.Count && index - 1 >= 0 && (index - 1) % width < index % width)
                {
                    p.Neighbors.Add(this.particles[index + width - 1]);
                    SpringDamper sd = new SpringDamper(p, this.particles[index + width - 1], this.ks, this.kd, this.lo);
                    GameObject obj = this.CreateDrawer(sd);
                    obj.transform.SetParent(drawObjHolder.transform);
                    this.Drawers.Add(obj);
                    this.springs.Add(sd);
                }

                // Top right node
                if (index + width + 1 < this.particles.Count && (index + 1) % width > index % width)
                {
                    p.Neighbors.Add(this.particles[index + width + 1]);
                    SpringDamper sd = new SpringDamper(p, this.particles[index + width + 1], this.ks, this.kd, this.lo);
                    GameObject obj = this.CreateDrawer(sd);
                    obj.transform.SetParent(drawObjHolder.transform);
                    this.Drawers.Add(obj);
                    this.springs.Add(sd);
                }
            }
        }

        /// <summary>
        /// The generate triangles.
        /// </summary>
        private void GenerateTriangles()
        {
            int width = (int)Mathf.Sqrt(this.numOfParticles);

            // First pass
            foreach (Particle p in this.particles)
            {
                int index = this.FindIndex(this.particles, p);

                // Set up the triangles with proper nodes
                if (index % width != width - 1 && index + width < this.particles.Count)
                {
                    Triangle t = new Triangle(this.particles[index], this.particles[index + 1], this.particles[index + width]);

                    foreach (SpringDamper sd in this.springs)
                    {
                        if ((sd.P1 == t.Particle1 && sd.P2 == t.Particle2) || (sd.P1 == t.Particle2 && sd.P2 == t.Particle1))
                        {
                            t.Sd1 = sd;
                        }
                        else if ((sd.P1 == t.Particle2 && sd.P2 == t.Particle3) || (sd.P1 == t.Particle3 && sd.P2 == t.Particle2))
                        {
                            t.Sd2 = sd;
                        }
                        else if ((sd.P1 == t.Particle3 && sd.P2 == t.Particle1) || (sd.P1 == t.Particle1 && sd.P2 == t.Particle3))
                        {
                            t.Sd3 = sd;
                        }
                    }
                    this.triangles.Add(t);
                }
            }

            // Second Pass
            foreach (Particle p in this.particles)
            {
                int index = this.FindIndex(this.particles, p);

                if (index >= width && index + 1 < this.particles.Count && index % width != width - 1)
                {
                    Triangle t = new Triangle(this.particles[index], this.particles[index + 1], this.particles[index - width + 1]);
                    this.triangles.Add(t);
                }
            }
        }

        /// <summary>
        /// Find the current index of the list.
        /// </summary>
        /// <param name="a_List">
        /// The list.
        /// </param>
        /// <param name="a_Item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int FindIndex(List<Particle> a_List, Particle a_Item)
        {
            int index = 0;

            // Get index
            for (int i = 0; i < a_List.Count; i++)
            {
                if (a_List[i] == a_Item)
                {
                    index = i;
                    break;
                }

            }

            return index;
        }

        /// <summary>
        /// TSet borders for the nodes .
        /// </summary>
        private void Borders()
        {
            // Move Particles
            foreach (Particle mp in this.particles)       
            {
                float x = mp.Force.x;
                float y = mp.Force.y;
                float z = mp.Force.z;

                Vector3 mousePosition = Camera.main.WorldToScreenPoint(mp.Position);

                // Floor
                if (mousePosition.y <= 10f) 
                {
                    if (mp.Force.y < 0f)
                    {
                        mp.Force = new Vector3(x, 0, z);
                    }
                    mp.Velocity = -mp.Velocity * .65f;
                }

                // Roof
                if (mousePosition.y > Screen.height - 10f)  
                {
                    if (mp.Force.y > 0f)
                    {
                        mp.Force = new Vector3(x, 0, z);
                    }
                    mp.Velocity = -mp.Velocity;
                }

                // Left Wall
                if (mousePosition.x < 10f)
                {
                    if (mp.Force.x < 0f)
                    {
                        mp.Force = new Vector3(0, y, z);
                    }
                    mp.Velocity = -mp.Velocity * .65f;
                }

                // Right Wall
                if (mousePosition.x > Screen.width - 10f)
                {
                    if (mp.Force.x > 0f)
                    {
                        mp.Force = new Vector3(0, y, z);
                    }
                    mp.Velocity = -mp.Velocity * .65f;
                }
            }
        }
    }
}
