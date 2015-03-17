using UnityEngine;
using System.Collections;

public class ISN_DeviceGUID  {

	private byte[] _Bytes = null;
	private string _Base64 = null;

	public ISN_DeviceGUID(string data) {
		_Base64 = data;
		_Bytes = System.Convert.FromBase64String(data);
	}


	public string GetBase64String() {
		return _Base64;
	}

	public byte[] GetBytes() {
		return _Bytes;
	}
}
