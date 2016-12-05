
namespace Assets.Scripts
{

using UnityEngine;

public class MonoParticle : MonoBehaviour {

    /// <summary>
    /// Reference to a particle.
    /// </summary>
    private Particle particle;

    /// <summary>
    /// Gets or sets the particle.
    /// </summary>
    public Particle ParticleRef
    {
            get { return particle; }
            set { particle = value; }
    }

    }
}
