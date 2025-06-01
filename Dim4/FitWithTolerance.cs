using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dim4
{
    public class FitWithTolerance
    {
        public SizeWithTolerance Hole { get; }
        public SizeWithTolerance Shaft { get; }

        public FitWithTolerance(SizeWithTolerance hole, SizeWithTolerance shaft)
        {
            if (hole.NominalSize != shaft.NominalSize)
                throw new ArgumentException("Номинальные размеры отверстия и вала должны совпадать");

            Hole = hole;
            Shaft = shaft;
        }

        /// <summary>
        /// Рассчитывает вид посадки: гарантированный зазор, гарантированный натяг или переходная посадка.
        /// </summary>
        public string GetFitType()
        {
            double minClearance = GetMinClearance();
            double maxClearance = GetMaxClearance();

            if (minClearance > 0)
                return "С гарантированным зазором";
            if (maxClearance < 0)
                return "С гарантированным натягом";
            return "Переходная (с зазором и с натягом)";
        }

        /// <summary>
        /// Рассчитывает минимальный (гарантированный) зазор или натяг.
        /// </summary>
        public double GetMinClearance()
        {
            return Hole.GetMinSize() - Shaft.GetMaxSize();
        }

        /// <summary>
        /// Рассчитывает максимальный зазор или натяг.
        /// </summary>
        public double GetMaxClearance()
        {
            return Hole.GetMaxSize() - Shaft.GetMinSize();
        }

        /// <summary>
        /// Рассчитывает средний зазор или натяг.
        /// </summary>
        public double GetMeanClearance()
        {
            double meanHole = (Hole.GetMaxSize() + Hole.GetMinSize()) / 2;
            double meanShaft = (Shaft.GetMaxSize() + Shaft.GetMinSize()) / 2;
            return meanHole - meanShaft;
        }
    }
}
