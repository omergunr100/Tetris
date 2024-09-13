using Management.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AdditionalContentButton : MonoBehaviour
    {
        private Image _image;
    
        void Start()
        {
            _image = GetComponentInParent<Image>();
            _image.color = SettingsManager.Instance.AdditionalContent ? Color.green : Color.red;
            SettingsManager.Instance.AddAdditionalContentListener(status => _image.color = status ? Color.green : Color.red);
        }
    }
}
