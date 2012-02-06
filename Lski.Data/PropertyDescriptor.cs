using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;

namespace Lski.Data {

	/// <summary>
	/// States an interface that should be used to create a custom, propertyDescriptorCollection (possibly with filters etc)
	/// </summary>
	/// <remarks></remarks>
	public interface IViewBuilder : ICloneable { 
		PropertyDescriptorCollection GetView(); 
	}

	/// <summary>
	/// A Generic implementation of the PropertyDescriptor Abstract Class.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks>A Generic implementation of the PropertyDescriptor Abstract Class. PropertyDescriptors can be used as a collection 
	/// 'PropertyDescriptorCollection' to provide a different view of the information that class contains.
	/// 
	/// For example: In my 'DataViewClass' a dynamically created collection of propertyDescriptor objects enable each row to be displayed
	/// to the calling code in the format desired, usually for a DataGridView, which accepts any BindingView object. The DataGridView object
	/// reads/writes the information to/from each of the DataViewRow objects, using this collection of PropertyDescriptor objects, rather than
	/// dealing with the DataViewRow directly.
	/// 
	/// NOTE: It is best practice to subclass these classes specfically for the class that they are to be used with. For instance 
	/// DataRowPropDescriptor subclasses PropBasicDescriptor to override the way it selects the correct property (which is a column position) in
	/// the DataViewRow it is used with.
	/// </remarks>
	public class PropBasicDescriptor<T> : PropertyDescriptor {

		protected Type _propertyType;

		public PropBasicDescriptor(string name, Type propertyType) : base(name, null) {	_propertyType = propertyType; }

		

		/// <summary>
		/// The datatype of the property being manipulated.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public override Type PropertyType {	get { return _propertyType; } }
		public override Type ComponentType { get { return typeof(T); } }
		public override bool CanResetValue(object component) { return false; }
		public override void ResetValue(object component) {}
		public override bool IsReadOnly { get { return false; } }

		public override object GetValue(object component) { return (T)component;}
		public override void SetValue(object component, object value) { component = value; }

		public override bool ShouldSerializeValue(object component) { return false; }
	}

	/// <summary>
	/// A Generic implementation of the PropertyDescriptor Abstract Class.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks>A Generic implementation of the PropertyDescriptor Abstract Class. PropertyDescriptors can be used as a collection 
	/// 'PropertyDescriptorCollection' to provide a different view of the information that class contains.
	///
	/// The difference between PropMethodDescriptor and PropBasicDescriptor, is that a delegate is passed to the class, to perform formatting
	/// on the property value before it is returned.
	/// 
	/// For example: In my 'DataViewClass' a dynamically created collection of propertyDescriptor objects enable each row to be displayed
	/// to the calling code in the format desired, usually for a DataGridView, which accepts any BindingView object. The DataGridView object
	/// reads/writes the information to/from each of the DataViewRow objects, using this collection of PropertyDescriptor objects, rather than
	/// dealing with the DataViewRow directly.
	/// 
	/// NOTE: It is best practice to subclass these classes specfically for the class that they are to be used with. For instance 
	/// DataRowPropDescriptor subclasses PropBasicDescriptor to override the way it selects the correct property (which is a column position) in
	/// the DataViewRow it is used with.
	/// </remarks>
	public class PropMethodDescriptor<T> : PropertyDescriptor {

		public delegate object GetValueMethod(T obj);

		protected GetValueMethod _getMethod;

		protected Type _propertyType;
		public PropMethodDescriptor(string name, GetValueMethod method, Type propertyType) : base(name, null) {
			
			_getMethod = method;
			_propertyType = propertyType;
		}

		public override object GetValue(object component) {	return _getMethod((T)component); }
		public override void SetValue(object component, object value) { component = value; }

		public override Type ComponentType { get { return typeof(T); } }
		public override Type PropertyType {	get { return _propertyType; } }

		public override bool CanResetValue(object component) { return false; }
		public override void ResetValue(object component) {}

		public override bool IsReadOnly { get { return false; } }
		public override bool ShouldSerializeValue(object component) { return false;}
	}

}
