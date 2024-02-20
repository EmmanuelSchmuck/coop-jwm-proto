using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Toolbox
{
	public static class NavmeshAgentExtensions
	{
		public static bool IsAtDestination(this NavMeshAgent navMeshAgent, float tolerancy = 0f)
		{
			//Debug.Log(navMeshAgent.remainingDistance == 0 && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete);
			//float remainingDistance = navMeshAgent.remainingDistance;
			return navMeshAgent.remainingDistance <= tolerancy && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete;
		}
	}
}