// ===========================================================================================================
//
// Class/Library: Unity Database - Base Class
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Mar 26, 2016
//	
// VERS 1.0.000 : Mar 26, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace CBT
{ 
	[System.Serializable]
	public class BaseDatabase<T> : ScriptableObject  where T: class
	{

		#region "PRIVATE VARIABLES"

			[SerializeField]
			public		List<T> database						= new List<T>();

			[System.NonSerialized]	
			private		bool		_blnDatabaseLoaded	= false;

		#endregion

		#region "PUBLIC PROPERTIES"

			public	int							Count
			{
				get
				{
					if (database == null)
							database = new List<T>();
					return database.Count;
				}
			}
			public	virtual		int		MaxID
			{
				get
				{
					if (database == null)
							database = new List<T>();

					return Count;
				}
			}
			public	bool						IsDatabaseLoaded
			{
				get
				{
					return _blnDatabaseLoaded;
				}
				set
				{
					_blnDatabaseLoaded = value;
				}
			}

		#endregion

		#region "PUBLIC METHODS"

			#if UNITY_EDITOR

			public	virtual		void		Add(				T				added)
			{
				database.Add(added);
				EditorUtility.SetDirty(this);
			}
			public	virtual		void		Insert(			int			index, T added)
			{
				database.Insert(index, added);
				EditorUtility.SetDirty(this);
			}
			public	void							Delete(			T				remove)
			{
				database.Remove(remove);
				EditorUtility.SetDirty(this);
			}
			public	void							Delete(			int			index)
			{
				database.RemoveAt(index);
				EditorUtility.SetDirty(this);
			}
			public	virtual		void		Save(				T				added)
			{
				// CREATE A CUSTOM SAVE METHOD
				EditorUtility.SetDirty(this);
			}
			public	virtual		void		Save(				int			index, T added)
			{
				if (index < 0)
				{
					Add(added);
				} else {
					database[index] = added;
					EditorUtility.SetDirty(this);
				}
			}

			#endif

			public	virtual		T				GetByIndex(	int			index)
			{
				try
				{
					return database.ElementAt(index);
				} catch {
					return null;
				}
			}
			public	virtual		T				GetByID(		int			intID)
			{
				int i = FindByID(intID);
				if (i >= 0)
					return GetByIndex(i);
				else
					return null;
			}
			public	T									GetByName(	string	name)
			{
				int i = FindByName(name);
				if (i >= 0)
					return GetByIndex(i);
				else
					return null;
			}

			public	virtual		void		Init()
			{ }
			protected					void		LoadDatabase<D, R>(string strPath, string strFile) where D : ScriptableObject where R : class
			{
				string strDBfullPath = @"Assets/" + strPath + "/" + strFile;
				D editorDB = null;		// ScriptableObject.CreateInstance<D>();

				if (!System.IO.Directory.Exists(@"Assets/" + strPath))
				{
					System.IO.Directory.CreateDirectory(@"Assets/" + strPath);
				} else {
					#if UNITY_EDITOR
						editorDB = AssetDatabase.LoadAssetAtPath(strDBfullPath, typeof(D)) as D;
					#else
						editorDB = Resources.GetBuiltinResource(typeof(D), strDBfullPath) as D;
					#endif

				}

				#if UNITY_EDITOR
				if (editorDB == null)
				{
					editorDB = ScriptableObject.CreateInstance<D>();
					AssetDatabase.CreateAsset(editorDB, strDBfullPath);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}
				#endif
				((BaseDatabase<R>)(object)editorDB).IsDatabaseLoaded = (editorDB != null);
				IsDatabaseLoaded = (editorDB != null);
			}

/*
			public	void							GetDatabase<T>(string strDBpath, string strDBname) where T : ScriptableObject
			{
				Debug.Log("Loading " + strDBname);
				T db = null;
				try
				{
					string strDBfullPath = @"Assets/" + strDBpath + "/" + strDBname;
					db = AssetDatabase.LoadAssetAtPath(strDBfullPath, typeof(T)) as T;

					if (db == null)
					{
						if (!AssetDatabase.IsValidFolder(@"Assets/" + strDBpath))
								 AssetDatabase.CreateFolder(@"Assets/", strDBpath);

						db = ScriptableObject.CreateInstance<T>();
						AssetDatabase.CreateAsset(db, strDBfullPath);
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
					}

					_blnDatabaseLoaded = (db != null);
					_strDBpath = strDBpath;
					_strDBname = strDBname;
				} catch {
					Debug.LogError("An Error occurred Loading the Database: " + strDBname);
				}
			}
*/

		#endregion

		#region "PRIVATE/PROTECTED FUNCTIONS"

			protected	virtual	int			FindByID(			int			intID)			// RETURN THE INDEX OF THE RECORD
			{
				if (intID < 1)
					return -1;

				// CREATE A CUSTOM FINDBYID METHOD

				return -1;
			}
			protected virtual int			FindByName(		string	strName)		// RETURN THE INDEX OF THE RECORD
			{
				if (strName.Trim() == "")
					return -1;

				// CREATE A CUSTOM FINDBYNAME METHOD

				return -1;
			}

		#endregion

	}
}
