///get_method(object,name)
/*
 * Returns a script index which has the name
   in form _[ObjectName]_[MethodName].
 * If no such method is found, methods for
   ancestor objects are searched instead.
 * If no ancestor has such method, -1 is returned. 
 */

var _object, _method;

//finding object's method, if there's one
_object = argument0;
_method = asset_get_index( "_" + object_get_name(_object) + "_" + argument1);

//checks for ancestors' methods, until one is found
while (_method == -1) {
    _object = object_get_parent(_object);
    _method = asset_get_index( "_" + object_get_name(_object) + "_" + argument1);
    
    if (_object == -1) return -1; //no method found, stop searching
    }
return _method;
