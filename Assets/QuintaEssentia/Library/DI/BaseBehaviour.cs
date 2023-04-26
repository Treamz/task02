using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QuintaEssenta.Library.DI
{
    public class BaseBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            GetComponentInThis();
        }

        private void GetComponentInThis()
        {
            var fields = GetFieldsWithAttribute(typeof(GetComponentAttribute));

            foreach (var field in fields)
            {
                var type = field.FieldType;
                var component = new UnityEngine.Object();

                if (type == gameObject.GetType())
                    component = gameObject;
                else component = GetComponent(type);

                if (component == null)
                {
                    throw new Exception($"GetComponent typeof(" + type.Name + ") in gameObject '" + gameObject.name + "' is null");
                }

                field.SetValue(this, component);
            }
        }

        public void InjectDependency(DIContainer container)
        {
            var fields = GetFieldsWithAttribute(typeof(InjectAttribute));

            foreach (var field in fields)
            {
                var type = field.FieldType;
                var component = new UnityEngine.Object();

                component = container.ReturnComponent(type);

                if (component == null)
                {
                    throw new Exception($"Inject component {type.Name} is null!");
                }

                field.SetValue(this, component);
            }
        }

        private IEnumerable<FieldInfo> GetFieldsWithAttribute(Type attributeType)
        {
            var fields = GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttributes(attributeType, true).FirstOrDefault() != null);

            return fields;
        }
    }
}
