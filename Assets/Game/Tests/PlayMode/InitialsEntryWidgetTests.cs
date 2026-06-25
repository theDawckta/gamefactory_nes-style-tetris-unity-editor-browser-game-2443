using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InitialsEntryWidgetTests
{
    private GameObject _go;
    private InitialsEntryWidget _widget;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("InitialsEntryWidget");
        _widget = _go.AddComponent<InitialsEntryWidget>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_ActivateSetsActiveTrue()
    {
        yield return null;
        _widget.Activate(0);
        Assert.IsTrue(_widget.IsActive);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_ActivateStoresScore()
    {
        yield return null;
        _widget.Activate(9999);
        Assert.AreEqual(9999, _widget.Score);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_ActivateResetsCharsToA()
    {
        yield return null;
        _widget.Activate(0);
        var chars = _widget.CurrentChars;
        Assert.AreEqual('A', chars[0]);
        Assert.AreEqual('A', chars[1]);
        Assert.AreEqual('A', chars[2]);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_ActivateSetsCursorToSlotZero()
    {
        yield return null;
        _widget.Activate(0);
        Assert.AreEqual(0, _widget.CursorSlot);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_ActivateClearsAwaitingConfirm()
    {
        yield return null;
        _widget.Activate(0);
        _widget.ConfirmSlot();
        _widget.ConfirmSlot();
        _widget.ConfirmSlot();
        Assert.IsTrue(_widget.AwaitingConfirm);
        _widget.Activate(0);
        Assert.IsFalse(_widget.AwaitingConfirm);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_DeactivateSetsActiveFalse()
    {
        yield return null;
        _widget.Activate(0);
        _widget.Deactivate();
        Assert.IsFalse(_widget.IsActive);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_CycleCharRightAdvancesChar()
    {
        yield return null;
        _widget.Activate(0);
        _widget.CycleChar(1);
        Assert.AreEqual('B', _widget.CurrentChars[0]);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_CycleCharLeftFromAWrapsToNine()
    {
        yield return null;
        _widget.Activate(0);
        _widget.CycleChar(-1);
        Assert.AreEqual('9', _widget.CurrentChars[0]);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_CycleCharRightFromNineWrapsToA()
    {
        yield return null;
        _widget.Activate(0);
        // cycle to '9' (index 35) by going left once
        _widget.CycleChar(-1);
        Assert.AreEqual('9', _widget.CurrentChars[0]);
        // cycle right from '9' wraps to 'A'
        _widget.CycleChar(1);
        Assert.AreEqual('A', _widget.CurrentChars[0]);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_CycleCharOnlyAffectsCurrentSlot()
    {
        yield return null;
        _widget.Activate(0);
        _widget.ConfirmSlot(); // move to slot 1
        _widget.CycleChar(1); // advance slot 1
        var chars = _widget.CurrentChars;
        Assert.AreEqual('A', chars[0]);
        Assert.AreEqual('B', chars[1]);
        Assert.AreEqual('A', chars[2]);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_ConfirmSlotAdvancesCursorToSlotOne()
    {
        yield return null;
        _widget.Activate(0);
        _widget.ConfirmSlot();
        Assert.AreEqual(1, _widget.CursorSlot);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_ConfirmSlotTwiceAdvancesCursorToSlotTwo()
    {
        yield return null;
        _widget.Activate(0);
        _widget.ConfirmSlot();
        _widget.ConfirmSlot();
        Assert.AreEqual(2, _widget.CursorSlot);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_ThirdConfirmSetsAwaitingConfirm()
    {
        yield return null;
        _widget.Activate(0);
        _widget.ConfirmSlot();
        _widget.ConfirmSlot();
        _widget.ConfirmSlot();
        Assert.IsTrue(_widget.AwaitingConfirm);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_GoBackReturnsToPreviousSlot()
    {
        yield return null;
        _widget.Activate(0);
        _widget.ConfirmSlot(); // slot 1
        _widget.GoBack();     // back to slot 0
        Assert.AreEqual(0, _widget.CursorSlot);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_GoBackOnSlotZeroDoesNothing()
    {
        yield return null;
        _widget.Activate(0);
        _widget.GoBack();
        Assert.AreEqual(0, _widget.CursorSlot);
    }

    [UnityTest]
    public IEnumerator InitialsEntryWidget_CycleCharAtoZSequence()
    {
        yield return null;
        _widget.Activate(0);
        // cycle through A-Z (26 steps right)
        for (int i = 0; i < 25; i++)
            _widget.CycleChar(1);
        Assert.AreEqual('Z', _widget.CurrentChars[0]);
        // one more -> '0'
        _widget.CycleChar(1);
        Assert.AreEqual('0', _widget.CurrentChars[0]);
    }
}
