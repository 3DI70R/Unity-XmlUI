using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public class ComponentVariableBinder : MonoBehaviour
    {
        [SerializeField]
        private BoundAttributeCollection attributeCollection;

        private IVariableProvider variableProvider;

        public void SetBoundAttrs(BoundAttributeCollection attributes)
        {
            attributeCollection?.UnbindFromProvider();
            attributeCollection = attributes;
            
            attributeCollection.ApplyConstants();

            if(variableProvider != null)
                attributeCollection?.BindToProvider(variableProvider);
        }

        public void SetVariableProvider(IVariableProvider provider)
        {
            variableProvider = provider;
            
            if(variableProvider != null)
                attributeCollection?.SetProvider(variableProvider);
        }

        private void Awake()
        {
            attributeCollection?.ApplyConstants();
        }

        private void OnDestroy()
        {
            attributeCollection?.UnbindFromProvider();
        }
    }
}