
namespace Assets.Scripts
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    /// <summary>
    /// Handles UI Activities - Singleton Pattern.
    /// </summary>
    public class UiManager : MonoBehaviour
    {
        /// <summary>
        /// The instance.
        /// </summary>
        private static UiManager sInstance;

        /// <summary>
        /// The wind toggle.
        /// </summary>
        [SerializeField]
        private Toggle windToggle;

        /// <summary>
        /// The wind resistance bar.
        /// </summary>
        [SerializeField]
        private Slider windResistanceBar;

        /// <summary>
        /// The spring constant bar.
        /// </summary>
        [SerializeField]
        private Slider springConstantBar;

        /// <summary>
        /// The damping factor bar.
        /// </summary>
        [SerializeField]
        private Slider dampingFactorBar;

        /// <summary>
        /// Gets the self.
        /// </summary>
        public static UiManager Self
        {
            get { return sInstance; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether wind.
        /// </summary>
        public bool Wind
        {
            get { return this.windToggle.isOn; }
            set { this.windToggle.isOn = value; }
        }

        /// <summary>
        /// Gets or sets the wind resistance.
        /// </summary>
        public float WindResistance
        {
            get { return this.windResistanceBar.value; }
            set { this.windResistanceBar.value = value; }
        }

        /// <summary>
        /// Gets or sets the spring constant.
        /// </summary>
        public float SpringConstant
        {
            get { return this.springConstantBar.value; }
            set { this.springConstantBar.value = value; }
        }

        /// <summary>
        /// Gets or sets the damping factor.
        /// </summary>
        public float DampingFactor
        {
            get { return this.dampingFactorBar.value; }
            set { this.dampingFactorBar.value = value; }
        }

        /// <summary>
        /// The restart.
        /// </summary>
        public void Restart()
        {
            SceneManager.LoadScene("Main");
        }

        /// <summary>
        /// The awake.
        /// </summary>
        private void Awake()
        {
            sInstance = this;
        }
    }
}
