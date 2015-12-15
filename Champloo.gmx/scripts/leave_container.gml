///leave_container()
/*
 * Removes calling component from its current container
   (if any).
 * Might be triggered explicitly, or when the container
   is destroyed.
 */


/* Unregistering component from container.
 * If the script was triggered by container being destroyed,
   this part is omitted as container data will be entirely
   removed, anyway.
 */
with (contained_by) {
    if (!is_destroyed) {
        script_execute(component_remover, other.id);
        layout_changed = true;
        }
    }

//unregistering container from component
contained_by = noone;