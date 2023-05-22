using CSF;

namespace XProject
{
    public class XModule : ModuleBase
    {
        [Command("helloworld")]
        public void HelloWorld()
        {
            Respond("Hello world!");
        }

        [Command("reply")]
        public void Reply([Remainder] string message)
        {
            Respond(message);
        }
    }
}