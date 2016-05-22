// ===========================================================================================================
//
// Class/Library: Class Builder - Database Script  (non-SQL database)
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
using System.Collections;
using System.Collections.Generic;

namespace CBT
{
	[System.Serializable]
	public class ClassBuilderDatabase : BaseDatabase<ClassBuilder> 
	{

		#region "PUBLIC PROPERTIES"

			public	override		int		MaxID
			{
				get
				{
					if (database == null)
							database = new List<ClassBuilder>();

					if (Count > 0)
						return database[Count - 1].ID;
					else 
						return 0;
				}
			}
			
		#endregion

		#region "PUBLIC METHODS"

			#if UNITY_EDITOR

			public	override		void						Add(				ClassBuilder				added)
			{
				added.ID = MaxID + 1;
				added.Index = Count;
				base.Add(added);
			}
			public	override		void						Insert(			int			index, ClassBuilder added)
			{
				added.ID = MaxID + 1;
				added.Index = index;
				base.Insert(index, added);
			}
			public	override		void						Save(				ClassBuilder				added)
			{
				if (added.ID < 1 || added.Index < 0)
				{
					int i = -1;
					if (added.ID > 0)
						i = FindByID(added.ID);
					if (i < 0)
						Add(added);
					else
					{
						added.Index = i;
						database[i] = added;
					}
				} else {
					database[added.Index] = added;
				}
				EditorUtility.SetDirty(this);
			}
			public	override		void						Save(				int			index, ClassBuilder added)
			{
				if (index < 0)
				{
					Add(added);
				} else {
					added.Index = index;
					database[index] = added;
					EditorUtility.SetDirty(this);
				}
			}

			#endif

			public	override		void						Init()
			{
				database = ClassBuilder.LoadDatabase().database;
			}
			public	override		ClassBuilder		GetByIndex(	int			index)
			{
				try
				{
					if (!IsDatabaseLoaded)
						database = ClassBuilder.LoadDatabase().database;

					if (database != null && (database.ElementAt(index)) != null)
							database.ElementAt(index).Index = index;
					else
							index = -1;
				} catch { index = -1; }


				if (index < 0)
					return new ClassBuilder();
				else
					return base.GetByIndex(index);
			}
			public	override		ClassBuilder		GetByID(		int			intID)
			{
				if (intID < 0)
					return null;

				if (!IsDatabaseLoaded)
					database = ClassBuilder.LoadDatabase().database;

				return database.Find(p => p.ID == intID);
			}

		#endregion

		#region "PRIVATE/PROTECTED FUNCTIONS"

			protected	override	int			FindByID(			int			intID)			// RETURN THE INDEX OF THE RECORD
			{
				if (intID < 1)
					return -1;
				int intRet = -1;
				for (int i = 0; i < Count; i++)
				{
					database.ElementAt(i).Index = i;
					if (database.ElementAt(i).ID == intID)
						intRet = i;
				}
				return intRet;
			}
			protected override	int			FindByName(		string	strName)		// RETURN THE INDEX OF THE RECORD
			{
				if (strName.Trim() == "")
					return -1;

				for (int i = 0; i < Count; i++)
				{
					database.ElementAt(i).Index = i;
					if (database.ElementAt(i).Name == strName)
						return i;
				}
				return -1;
			}

		#endregion

	}
}
