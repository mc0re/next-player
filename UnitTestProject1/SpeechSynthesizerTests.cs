using System;
using System.IO;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VoiceControlLibrary;


namespace VoiceControlLibraryTests
{
    [TestClass]
    public class SpeechSynthesizerTests
    {
        [TestMethod]
        public void Speak_Disabled()
        {
            var vc = new Mock<IVoiceConfiguration>().SetupProperty(v => v.IsVoiceControlEnabled, false);
            var pl = new Mock<IVoicePlayer>();
            pl.Setup(p => p.PlayAndForget(It.IsAny<Stream>(), It.IsAny<int>())).Throws(new ArgumentException("Should not run"));

            InterfaceMapper.SetInstance(vc.Object);
            InterfaceMapper.SetInstance(pl.Object);

            var sut = new SpeechSynthesizerControl();
            sut.Setup();
            sut.Speak("Test");
            // Does not throw - means the player was not called.
        }


        [TestMethod]
        public void Speak_David()
        {
            var vc = new Mock<IVoiceConfiguration>()
                    .SetupProperty(v => v.IsVoiceControlEnabled, true)
                    .SetupProperty(v => v.VoiceControlFeedbackChannel, 5)
                    .SetupProperty(v => v.VoiceControlFeedbackVoice, "Microsoft David Desktop");
            var pl = new Mock<IVoicePlayer>();
            
            var channel = 0;
            Stream input = null;
            pl.Setup(p => p.PlayAndForget(It.IsAny<Stream>(), It.IsAny<int>()))
                .Callback<Stream, int>((s, ch) => { input = s; channel = ch; });

            InterfaceMapper.SetInstance(vc.Object);
            InterfaceMapper.SetInstance(pl.Object);

            var sut = new SpeechSynthesizerControl();
            sut.Setup();
            sut.Speak("Test");
            Assert.AreEqual(5, channel);
            Assert.AreEqual(116816, input?.Length);
        }
    }
}
