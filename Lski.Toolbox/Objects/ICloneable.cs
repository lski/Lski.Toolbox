﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Toolbox.Objects {

	public interface ICloneable<T> {
		T Clone();
	}
}