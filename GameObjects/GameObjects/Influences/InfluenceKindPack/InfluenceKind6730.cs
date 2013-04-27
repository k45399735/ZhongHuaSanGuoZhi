﻿namespace GameObjects.Influences.InfluenceKindPack
{
    using GameObjects;
    using GameObjects.Influences;
    using System;

    internal class InfluenceKind6730 : InfluenceKind
    {
        private int increment;
        private int prob;

        public override void ApplyInfluenceKind(Troop t)
        {
            t.PoliticsDecreaseProb += prob;
            t.PoliticsDecrease += increment;
        }

        public override void PurifyInfluenceKind(Troop t)
        {
            t.PoliticsDecreaseProb -= prob;
            t.PoliticsDecrease -= increment;
        }

        public override void InitializeParameter(string parameter)
        {
            try
            {
                this.prob = int.Parse(parameter);
            }
            catch
            {
            }
        }

        public override void InitializeParameter2(string parameter)
        {
            try
            {
                this.increment = int.Parse(parameter);
            }
            catch
            {
            }
        }
    }
}
