using UnityEngine;

public interface IPickupable
{
   void OnPickup();
   GameObject Origin { get; set; }
  ObjectPlaceholder ObjectPlaceholder { get; set; }
}
