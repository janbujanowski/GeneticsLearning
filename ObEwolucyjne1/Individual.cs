namespace ObEwolucyjne1
{
    public class Individual
    {
        public uint genotype;

        public double Fenotype
        {
            get
            {
                return -2 + genotype / Environment.DIVIDER;
            }
        }

        public double SurvivalScore
        {
            get
            {
                return Environment.SurvivalFunction(Fenotype);
            }
        }
        public override string ToString()
        {
            return $"Fenotype : {Fenotype} with score : {SurvivalScore}";
        }
    }
}
