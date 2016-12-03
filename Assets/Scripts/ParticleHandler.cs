
namespace Assets.Scripts
{
    using UnityEngine;

    /// <summary>
    /// The particle handler.
    /// </summary>
    public class ParticleHandler : MonoBehaviour
    {
        /// <summary>
        /// The current particle reference.
        /// </summary>
        private GameObject current = null;

        /// <summary>
        /// The update.
        /// </summary>
        private void Update()
        { // If left mouse click
            if (Input.GetMouseButtonDown(0))
            { // If the ray hits an object and its a particle that is not null
                if (this.ShootRay() != null && this.ShootRay().GetComponent<Particle>() != null)
                { // Set current object to the returned object
                    this.current = this.ShootRay();

                    // Set the particles anchor to opposite of what it currently is
                    this.current.GetComponent<Particle>().Anchor = this.current.GetComponent<Particle>().Anchor != true;
                }
                else
                { // Set to null
                    this.current = null;
                }
            }
        }

        /// <summary>
        /// The late update.
        /// </summary>
        private void LateUpdate()
        {
            // If the mouse button is held down and the object is not null
            if (Input.GetMouseButton(0) && this.current != null)
            {// Set the particles position to the position of the mouse..this allows dragging
                Vector3 mouse = Input.mousePosition;
                mouse.z = -Camera.main.transform.position.z;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouse);
                worldPos.z = this.current.transform.position.z;
                this.current.transform.position = worldPos;
            }
        }

        /// <summary>
        /// The shoot ray.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/>.
        /// </returns>
        private GameObject ShootRay()
        {// Get the ray position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // New raycasthit instance
            RaycastHit hit = new RaycastHit();

            // Cast out the ray
            Physics.Raycast(ray.origin, ray.direction, out hit);

            // If it hits something that is not null then return what it hit
            if (hit.transform != null)
            {
                return hit.transform.gameObject;
            }

            return null;
        }
    }
}
