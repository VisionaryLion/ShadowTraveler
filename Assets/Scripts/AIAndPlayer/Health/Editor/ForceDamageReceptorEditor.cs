using UnityEditor;

/*
Author: Oribow
*/
namespace Combat
{
    [CustomEditor(typeof(ForceDamageReceptor))]
    public class ForceDamageReceptorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ForceDamageReceptor myTarget = (ForceDamageReceptor)target;
            myTarget.health = (IHealth)EditorGUILayout.ObjectField(myTarget.health, typeof(IHealth), true);
            myTarget.directionDamageMultiplier = EditorGUILayout.Vector3Field("dir damage multiplier",myTarget.directionDamageMultiplier);
            myTarget.enableSurfaceMultipliers = EditorGUILayout.Toggle("diffrently weight surfacetyps", myTarget.enableSurfaceMultipliers);
            if (myTarget.enableSurfaceMultipliers)
            {
                if(myTarget.surfaceMultiplier.Length != SurfaceTypeUser.SurfaceTypes.SurfaceTypeCount)
                    myTarget.surfaceMultiplier = new float[SurfaceTypeUser.SurfaceTypes.SurfaceTypeCount];
                EditorGUI.indentLevel = 1;
                for (int i = 0; i < myTarget.surfaceMultiplier.Length; i++)
                {
                    myTarget.surfaceMultiplier[i] = EditorGUILayout.FloatField(SurfaceTypeUser.SurfaceTypes.SurfaceTypeToString(i), myTarget.surfaceMultiplier[i]);
                }
                EditorGUI.indentLevel = 0;
            }
            myTarget.minImpactMag = EditorGUILayout.FloatField("damage threshold", myTarget.minImpactMag);
        }
    }
}
