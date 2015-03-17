////////////////////////////////////////////////////////////////////////////////
//  
// @module Events Pro
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////
 


namespace UnionAssets.FLE {

	public class CEvent {
		private int _id;
		private string _name;
		private object _data;

		private IDispatcher _dispatcher;
		private bool _isStopped = false;
		private bool _isLocked = false;


		public object _currentTarget;


		//--------------------------------------
		// INITIALIZE
		//--------------------------------------

		public CEvent(int id, string name, object data, IDispatcher dispatcher) {
			_id = id;
			_name = name;
			_data = data;
			_dispatcher = dispatcher;
		}


		//--------------------------------------
		// PUBLIC METHODS
		//--------------------------------------
		

		public void stopPropagation() {
			_isStopped = true;
		}

		public void stopImmediatePropagation() {
			_isStopped = true;
			_isLocked = true;
		}

		public bool canBeDispatched(object val) {
			if(_isLocked) {
				return false;
			}

			if(_isStopped) {
				if(_currentTarget == val) {
					return true;
				} else {
					return false;
				}
			} else {
				_currentTarget = val;
				return true;
			}
		}

		


		//--------------------------------------
		// GET / SET
		//--------------------------------------

		public int id {
			get {
				return _id;
			}
		}

		public string name {
			get {
				return _name;
			}
		}

		public object data {
			get {
				return _data;
			}
		}

		public IDispatcher target {
			get {
				return _dispatcher;
			}
		}

		public IDispatcher dispatcher {
			get {
				return _dispatcher;
			}
		}


		public object currentTarget {
			get {
				return _currentTarget;
			}
		}
		


		public bool isStopped {
			get {
				return _isStopped;
			}
		}


		public bool isLocked {
			get {
				return _isLocked;
			}
		}


	}

}
