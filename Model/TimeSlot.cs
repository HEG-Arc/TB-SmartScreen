namespace SCE_ProductionChain.Model
{
    public class TimeSlot
    {
        private bool isWorking;
        private int duration;

        public TimeSlot(bool isWorking, int duration)
        {
            this.isWorking = isWorking;
            this.duration = duration;
        }

        public bool IsWorking
        {
            get
            {
                return isWorking;
            }

            set
            {
                isWorking = value;
            }
        }

        public int Duration
        {
            get
            {
                return duration;
            }

            set
            {
                duration = value;
            }
        }
    }
}
