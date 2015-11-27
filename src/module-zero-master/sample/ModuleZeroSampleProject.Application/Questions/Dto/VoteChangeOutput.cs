namespace ModuleZeroSampleProject.Questions.Dto
{
    public class VoteChangeOutput
    {
        public int VoteCount { get; set; }

        public VoteChangeOutput()
        {
            
        }

        public VoteChangeOutput(int voteCount)
        {
            VoteCount = voteCount;
        }
    }
}