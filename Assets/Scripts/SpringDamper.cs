
namespace Assets.Scripts
{
    using UnityEngine;

    /// <summary>
    /// The spring damper.
    /// </summary>
    public class SpringDamper
    {
        /// <summary>
        /// The spring constant.
        /// </summary>
        private float ks;

        /// <summary>
        /// The Damping Factor.
        /// </summary>
        private float kd;
  
        /// <summary>
        /// The rest length.
        /// </summary>
        private float lo;

        /// <summary>
        /// The Spring = -ks * (lo-l).
        /// </summary>
        private float spring;

        /// <summary>
        /// The damping = -KD * (v1-v2).
        /// </summary>
        private float damping;

        /// <summary>
        /// The spring damper = -ks(lo-l) * -KD(v1-v2).
        /// </summary>
        private Vector3 springDamper;

        /// <summary>
        /// Particle 1.
        /// </summary>
        private Particle p1;

        /// <summary>
        /// Particle 2.
        /// </summary>
        private Particle p2;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpringDamper"/> class.
        /// </summary>
        /// <param name="a_Pa1">
        /// The first particle to be in the damper.
        /// </param>
        /// <param name="a_Pa2">
        /// The The second particle to be in the damper.
        /// </param>
        /// <param name="a_Springconstant">
        /// The spring constant factor.
        /// </param>
        /// <param name="a_Springd">
        /// The Spring Damping factor.
        /// </param>
        /// <param name="a_Restlength">
        /// The RestLength.
        /// </param>
        public SpringDamper(Particle a_Pa1, Particle a_Pa2, float a_Springconstant, float a_Springd, float a_Restlength)
        {
            this.p1 = a_Pa1;
            this.p2 = a_Pa2;

            this.ks = a_Springconstant;
            this.kd = a_Springd;
            this.lo = a_Restlength;
        }

        /// <summary>
        /// Gets or sets the ks.
        /// </summary>
        public float Ks
        {
            get { return this.ks; }
            set { this.ks = value; }
        }

        /// <summary>
        /// Gets or sets the variable.
        /// </summary>
        public float Kd
        {
            get { return this.kd; }
            set { this.kd = value; }
        }

        /// <summary>
        /// Gets or sets the lo.
        /// </summary>
        public float Lo
        {
            get { return this.lo; }
            set { this.lo = value; }
        }

        /// <summary>
        /// Gets or sets the p 1.
        /// </summary>
        public Particle P1
        {
            get { return this.p1; }
            set { this.p1 = value; }
        }

        /// <summary>
        /// Gets or sets the p 2.
        /// </summary>
        public Particle P2
        {
            get { return this.p2; }
            set { this.p2 = value; }
        }

        /// <summary>
        /// Compute the forces to act upon the particles.
        /// </summary>
        public void ComputeForces()
        {
            Vector3 displacement = this.p2.Position - this.p1.Position;

            Vector3 direction = displacement / displacement.magnitude;

            // 1D velocities
            float v1D = Vector3.Dot(direction, this.p1.Velocity);
            float v2D = Vector3.Dot(direction, this.p2.Velocity);

            // Spring force
            this.spring = -this.ks * (this.lo - displacement.magnitude);

            // Damping force
            this.damping = -this.kd * (v1D - v2D);

            // Spring force 3D
            this.springDamper = (this.spring + this.damping) * direction;

            this.p1.AddForce(this.springDamper);
            this.p2.AddForce(-this.springDamper);

        }

        /// <summary>
        /// Check if the break happens.
        /// </summary>
        /// <param name="a_BreakMultiplyer">
        /// The break factor.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool BreakHappens(float a_BreakMultiplyer)
        {
            // If true then we will be removing the neighbor from the particle
            if ((this.p2.transform.position - this.p1.transform.position).magnitude > this.lo * a_BreakMultiplyer)
            {
                if (this.p2.Neighbors.Contains(this.p1))
                {
                    this.p2.Neighbors.Remove(this.p1);
                }
                if (this.p1.Neighbors.Contains(this.p2))
                {
                    this.p1.Neighbors.Remove(this.p2);
                }

                return true;
            }

            return false;
        }
    }
}
