namespace Penguin
{
    public class TestPhrases
    {
        public const string UncleanedPhrase = "pls  fix    me ,>% I have hard  !times splling .";
        public const string TestPhrase1 = "remove all of the 883 blocks and replace those with 993 blocks then delete them thanks bye. replace all 883 with 993  Oh sorry could you move all of those blocks right seven blocks? Wait wait sorry cancel that last thing";
        public const string TestPhrase2 = "hey bot your cool can you delete all of the red candy lifesaver blocks and yeah totally replace those with swoards";
        public const string TestPhrase3 = "reaplace all of the red plastic blocks with orange ones.";


        public const string IsCommand = "move all of the right blocks right 2 blocks"; //In this example right should be tokenized as a command

        public const string IsBlock = "replace the basic gray blocks with left arrows."; //In this example left should be tokenized as a block

        public const string IsBlock2 = "move all the left booster blocks left 8 blocks left"; // In this example left booster should be tokenized as a command

         public const string IsBlock3 = "move some no just kidding all of the blocks that are left left 2 blocks"; // In this example left is applied twice but since it is refering to the block the first instance the second instance is assumed to be a direction
    }
}
