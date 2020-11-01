using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public class ComponentVariableBinder : MonoBehaviour
    {
        [SerializeField]
        private BoundVariableCollection attributeCollection;

        private IVariableProvider variableProvider;

        public void SetBoundAttrs(BoundVariableCollection attributes)
        {
            attributeCollection?.UnbindFromProvider();
            attributeCollection = attributes;
            
            if(variableProvider != null)
                attributeCollection?.BindToProvider(variableProvider);
        }

        public void SetVariableProvider(IVariableProvider provider)
        {
            variableProvider = provider;
            
            if(variableProvider != null)
                attributeCollection?.SetProvider(variableProvider);
        }

        private void OnDestroy()
        {
            attributeCollection?.UnbindFromProvider();
        }
    }
}