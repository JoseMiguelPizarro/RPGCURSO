using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class EXPTEST {

    

    [Test]
    public void PruebaPrueba() {
        // Use the Assert class to test conditions.
        Assert.AreEqual(1, 1);
    }

   [Test]
   public void pruebaexp1()
    {

    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses() {
        
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
