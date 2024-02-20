using System;
using System.Collections;
using System.Collections.Generic;

namespace Toolbox
{
	public class EventData : System.EventArgs
	{

	}
	public delegate void GenericEventHandler<T, U>(T sender, U u) where U : EventData;

}