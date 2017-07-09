namespace TomiSoft.MP3Player.Utils {
    public class LongOperationProgress {
        public long Maximum {
            get;
            set;
        }

        public long Position {
            get;
            set;
        }

        public bool IsIndetermine {
            get;
            set;
        }

        public int Percent {
            get {
                return (int)((double)this.Position / this.Maximum * 100);
            }
        }
    }
}
