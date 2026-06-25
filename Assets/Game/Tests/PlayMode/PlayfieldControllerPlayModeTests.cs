using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayfieldControllerPlayModeTests
{
    [UnityTest]
    public IEnumerator PlayfieldController_AwakeInitializesGrid()
    {
        var go = new GameObject();
        var controller = go.AddComponent<PlayfieldController>();
        yield return null;

        Assert.AreEqual(0, controller.GetCell(0, 0));
        Assert.AreEqual(0, controller.GetCell(21, 9));

        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator PlayfieldController_SetAndGetCellRoundTrip()
    {
        var go = new GameObject();
        var controller = go.AddComponent<PlayfieldController>();
        yield return null;

        controller.SetCell(5, 5, 4);
        Assert.AreEqual(4, controller.GetCell(5, 5));

        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator PlayfieldController_ClearAllResetsAfterSet()
    {
        var go = new GameObject();
        var controller = go.AddComponent<PlayfieldController>();
        yield return null;

        controller.SetCell(0, 0, 7);
        controller.ClearAll();
        Assert.AreEqual(0, controller.GetCell(0, 0));

        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator PlayfieldController_HasPlayfieldControllerComponent()
    {
        var go = new GameObject();
        go.AddComponent<PlayfieldController>();
        yield return null;

        Assert.IsNotNull(go.GetComponent<PlayfieldController>());

        Object.Destroy(go);
    }
}
