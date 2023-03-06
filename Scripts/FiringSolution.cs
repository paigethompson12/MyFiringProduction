using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringSolution
{
    //needs to be nullable in order to set it to null - or else there's an error
    public Nullable<Vector3> calculateFiringSolution(Vector3 start, Vector3 end, float muzzleV, Vector3 gravity)
    {
        //calculate vector from the target back to the start
        Vector3 delta = start - end;

        //calculate the real-valued a,b,c coefficients
        //of a conventional quadratic equation
        float a = Mathf.Pow(gravity.magnitude,2);
        float b = -4 * (Vector3.Dot(gravity, delta) + muzzleV * muzzleV);
        float c = 4 * Mathf.Pow(delta.magnitude,2);

        //check for no real solutions
        float b2minus4ac  = b * b - 4 * a * c;
		if (b2minus4ac < 0)
			return null;

        //find the candidate times
        float time0 = Mathf.Sqrt((-b + Mathf.Sqrt(b2minus4ac)) / (2 * a));
        float time1 = Mathf.Sqrt((-b - Mathf.Sqrt(b2minus4ac)) / (2 * a));

        //find the time to target
        Nullable<float> ttt = 0;
        if(time0 < 0)
        {
            if(time1 < 0)
                return null;
            else
                ttt = time1;
        }
        else
        {
            if(time1 < 0)
                ttt = time0;
            else
                ttt = Mathf.Min(time0, time1);
        }

        if (!ttt.HasValue)
            return null;

        delta = end - start;

        //return the firing vector
        Debug.Log("solution: " + (delta * 2 - gravity * ttt * ttt) / (2 * muzzleV * ttt));
        return (delta * 2 - gravity * ttt * ttt) / (2 * muzzleV * ttt);
    }

}