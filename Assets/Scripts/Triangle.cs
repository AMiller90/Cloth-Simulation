
namespace Assets.Scripts
{
    using UnityEngine;

    /// <summary>
    /// Class to represent Triangle objects.
    /// </summary>
    public class Triangle
    {

        /// <summary>
        /// The first particle of the triangle.
        /// </summary>
        private readonly Particle pone;

        /// <summary>
        /// The second particle of the triangle.
        /// </summary>
        private readonly Particle ptwo;

        /// <summary>
        /// The third particle of the triangle.
        /// </summary>
        private readonly Particle pthree;

        /// <summary>
        /// The area.
        /// </summary>
        private float area;

        /// <summary>
        /// The aero force.
        /// </summary>
        private Vector3 aeroForce;

        /// <summary>
        /// The velocity.
        /// </summary>
        private Vector3 velocity;

        /// <summary>
        /// The normal.
        /// </summary>
        private Vector3 normal;

        /// <summary>
        /// The first spring damper of the triangle.
        /// </summary>
        private SpringDamper sd1;

        /// <summary>
        /// The second spring damper of the triangle.
        /// </summary>
        private SpringDamper sd2;

        /// <summary>
        /// The third spring damper of the triangle.
        /// </summary>
        private SpringDamper sd3;

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle"/> class.
        /// </summary>
        /// <param name="a_P1">
        /// The p 1.
        /// </param>
        /// <param name="a_P2">
        /// The p 2.
        /// </param>
        /// <param name="a_P3">
        /// The p 3.
        /// </param>
        public Triangle(Particle a_P1, Particle a_P2, Particle a_P3)
        {
            this.pone = a_P1;
            this.ptwo = a_P2;
            this.pthree = a_P3;
        }

        /// <summary>
        /// Gets the particle 1.
        /// </summary>
        public Particle Particle1
        {
            get { return this.pone; }
        }

        /// <summary>
        /// Gets the particle 2.
        /// </summary>
        public Particle Particle2
        {
            get { return this.ptwo; }
        }

        /// <summary>
        /// Gets the particle 3.
        /// </summary>
        public Particle Particle3
        {
            get { return this.pthree; }
        }

        /// <summary>
        /// Gets or sets the first Spring Damper.
        /// </summary>
        public SpringDamper Sd1
        {
            get { return this.sd1; }
            set { this.sd1 = value; }
        }

        /// <summary>
        /// Gets or sets the second Spring Damper.
        /// </summary>
        public SpringDamper Sd2
        {
            get { return this.sd2; }
            set { this.sd2 = value; }
        }

        /// <summary>
        /// Gets or sets the third Spring Damper.
        /// </summary>
        public SpringDamper Sd3
        {
            get { return this.sd3; }
            set { this.sd3 = value; }
        }

        /// <summary>
        /// Calculate the Aerodynamic force.
        /// </summary>
        /// <param name="a_Wind">
        /// The a_ wind.
        /// </param>
        public void CalcAeroForce(Vector3 a_Wind)
        {
            Vector3 surface = (this.pone.Velocity + this.ptwo.Velocity + this.pthree.Velocity) / 3f;

            this.velocity = surface - a_Wind;

            // Caluclate the normal
            Vector3 crossed = Vector3.Cross(this.ptwo.Position - this.pone.Position, this.pthree.Position - this.pone.Position);
            this.normal = crossed / crossed.magnitude;

            // Calculate area
            float areaHolder = (1f / 2f) * crossed.magnitude;
            this.area = areaHolder * (Vector3.Dot(this.velocity, this.normal) / this.velocity.magnitude);

            // Calculate AeroForce
            this.aeroForce = -(1f / 2f) * 1f * Mathf.Pow(this.velocity.magnitude, 2) * 1f * this.area * this.normal;
        
            // Add force to each particle of the triangle
            this.pone.AddForce(this.aeroForce / 3);
            this.ptwo.AddForce(this.aeroForce / 3);
            this.pthree.AddForce(this.aeroForce / 3);
        }
    }
}
