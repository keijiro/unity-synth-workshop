namespace Stk
{
    public struct StereoFrame
    {
        public float left;
        public float right;

        public float Mono {
            get { return 0.5f * (left + right); }
        }

        public StereoFrame (float left, float right)
        {
            this.left = left;
            this.right = right;
        }
    }
}