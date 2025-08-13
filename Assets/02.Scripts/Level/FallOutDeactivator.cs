using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallOutDeactivator : MonoBehaviour
{
    [SerializeField] private float deactiveHeight = -5;

    private void Update()
    {
        if (transform.position.y <= deactiveHeight)
            gameObject.SetActive(false);
    }
}
