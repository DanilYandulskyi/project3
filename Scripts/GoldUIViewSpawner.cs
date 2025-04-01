using UnityEngine;

public class GoldUIViewSpawner : MonoBehaviour
{
    [SerializeField] private GoldUIView _goldUIView;

    public GoldUIView Spawn(Transform transform)
    {
        GoldUIView goldUIView = Instantiate(_goldUIView, transform);

        return goldUIView;
    }
}
