
namespace Assets.Scripts
{
    using System.Collections.Generic;

    using UnityEngine;

    /// <summary>
    /// The particle.
    /// </summary>
    public class Particle
    {
        /// <summary>
        /// The velocity.
        /// </summary>
        [SerializeField]
        private Vector3 velocity;

        /// <summary>
        /// The position.
        /// </summary>
        private Vector3 position;

        /// <summary>
        /// The acceleration.
        /// </summary>
        private Vector3 acceleration;

        /// <summary>
        /// The force.
        /// </summary>
        private Vector3 force;

        /// <summary>
        /// The anchor.
        /// </summary>
        private bool anchor;

        /// <summary>
        /// The List for the neighbor particles to current particle.
        /// </summary>
        private List<Particle> neighbors;

        /// <summary>
        /// The index.
        /// </summary>
        private int index;

        /// <summary>
        /// The mass.
        /// </summary>
        private float mass;

        /// <summary>
        /// Initializes a new instance of the <see cref="Particle"/> class.
        /// </summary>
        /// <param name="a_Position">
        /// The position to be set
        /// </param>
        public Particle(Vector3 a_Position)
        {
            this.position = a_Position;
            this.velocity = Vector3.zero;
            this.force = Vector3.zero;
            this.mass = 1;
            this.anchor = false;
        }

        /// <summary>
        /// Gets or sets the mass.
        /// </summary>
        public float Mass
        {
            get { return this.mass; }
            set { this.mass = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether anchor.
        /// </summary>
        public bool Anchor
        {
            get { return this.anchor; }
            set { this.anchor = value; }
        }

        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        public Vector3 Velocity
        {
            get { return this.velocity; }
            set { this.velocity = value; }
        }

        /// <summary>
        /// Gets or sets the acceleration.
        /// </summary>
        public Vector3 Acceleration
        {
            get { return this.acceleration; }
            set { this.acceleration = value; }
        }

        /// <summary>
        /// Gets or sets the force.
        /// </summary>
        public Vector3 Force
        {
            get { return this.force; }
            set { this.force = value; }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets or sets the neighbors.
        /// </summary>
        public List<Particle> Neighbors
        {
            get { return this.neighbors; }
            set { this.neighbors = value; }
        }

        /// <summary>
        /// The add force.
        /// </summary>
        /// <param name="a_F">
        /// The f.
        /// </param>
        public void AddForce(Vector3 a_F)
        {
            this.force += a_F;
        }

        /// <summary>
        /// The update particle.
        /// </summary>
        public Vector3 UpdateParticle()
        {
            // Get acceleration
            this.acceleration = (1f / this.mass) * this.force;

            // Apply to velocity 
            this.velocity += this.acceleration * Time.fixedDeltaTime;

            // Set the velocity
            this.velocity = Vector3.ClampMagnitude(this.velocity, this.velocity.magnitude);

            // Move the particle
            return this.position += this.velocity * Time.fixedDeltaTime;
        }
    }
}
