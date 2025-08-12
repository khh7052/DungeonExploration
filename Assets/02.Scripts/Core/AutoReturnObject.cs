using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReturnObject : MonoBehaviour
{
    private void OnDisable()
    {
        ObjectPoolingManager.Instance.Return(gameObject);
    }
}
