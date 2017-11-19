using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Elastic : MonoBehaviour
{
	public Transform		from;
	public Vector3			fromOffset;
	public Transform		to;
	public Vector3			toOffset;

	Vector3					originalScale;

	void Start ()
	{
		originalScale = transform.localScale;
	}

    void Update()
    {
        var pA = from.position + fromOffset;
        var pB = to.position + toOffset;
        transform.position = (pA + pB) / 2;
		var b = (pA.y > pB.y) ? pA - pB : pB - pA;
		transform.eulerAngles = new Vector3(0, 0, Vector3.Angle(Vector3.right, b));
        var scale = originalScale;
		scale.y = .5f;
        scale.x = Vector3.Distance(pA, pB) / 2;
        transform.localScale = scale;
    }
}
