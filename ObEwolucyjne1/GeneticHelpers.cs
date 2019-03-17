namespace ObEwolucyjne1
{
    public static class GeneticHelpers
    {
        public static uint Sum(this uint[] arrayOfStuff)
        {
            uint sum = 0;
            foreach (var item in arrayOfStuff)
            {
                sum += item;
            }
            return sum;
        }

        public static Individual GetRandomParent(this Population population)
        {
            var x = Environment.CUBE.Next(0, population.genotypes.Length);
            var y = Environment.CUBE.Next(0, population.genotypes.Length);
            if (population.genotypes[x].SurvivalScore >= population.genotypes[y].SurvivalScore)
            {
                return population.genotypes[x];
            }
            else
            {
                return population.genotypes[y];
            }
        }

    }
}