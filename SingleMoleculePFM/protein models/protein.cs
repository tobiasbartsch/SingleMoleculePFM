﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleMoleculePFM.protein_models
{
    public interface protein
    {
        double Protenergy(double z);
        //double PropagateFolding(double force, double dt, double dx);
    }
}
