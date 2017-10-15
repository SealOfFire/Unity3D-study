using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirection
{
	NE,
	E,
	SE,
	SW,
	W,
	NW
}

/// <summary>
/// 获取相反方向
/// </summary>
public static class HexDirectionExtensions
{
	public static HexDirection Opposite (this HexDirection direction)
	{ 
		return (int)direction < 3 ? (direction + 3) : (direction - 3); 
	}
} 