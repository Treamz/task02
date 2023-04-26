using UnityEngine;
using System.Collections.Generic;
using QuintaEssenta.Library.DI;

[ExecuteInEditMode]
public class DIContainer : MonoBehaviour
{
    private List<BaseBehaviour> _baseBehaviours = new List<BaseBehaviour>();

    private void Awake()
    {
        FindAllBaseBehaviours();
        InjectDependecies();
    }

    private void FindAllBaseBehaviours()
    {
        _baseBehaviours.Clear();

        Object[] objects = Resources.FindObjectsOfTypeAll(typeof(BaseBehaviour));

        foreach (var item in objects)
        {
            _baseBehaviours.Add((BaseBehaviour)item);
        }
    }

    private void InjectDependecies()
    {
        for (int i = 0; i < _baseBehaviours.Count; i++)
        {
            _baseBehaviours[i].InjectDependency(this);
        }
    }

    public Component ReturnComponent(System.Type type)
    {
        if (type.IsInterface)
        {
            for (int i = 0; i < _baseBehaviours.Count; i++)
            {
                if (_baseBehaviours[i].TryGetComponent(type, out Component component))
                {
                    return component;
                }
            }
        }

        var finded = FindObjectOfType(type);

        Debug.Log($"Finded: finded");

        Component newCompoent = (Component)finded;
        return newCompoent;

        throw new System.Exception($"Component {type} not found in DI container!");
    }
}
