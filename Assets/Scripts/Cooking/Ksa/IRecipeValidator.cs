using System.Collections;
using System.Collections.Generic;
using Items.Core;
using UnityEngine;

// Owner: Ksa (serving/recipes)
// Purpose: Contract used by Serving to validate identity (base + method) and compute mistake penalties for doneness.
// Notes: ServingStation calls this before crafting the plate; UI remains dumb.

public interface IRecipeValidator
{
    /// <summary>
    /// Revisa que los ingredientes sean los correctos y usen los Cooking Methods correctos
    /// Si es falso no tiene que devolver plato o devlolver un plato basura.
    /// </summary>
    bool ValidateIdentity(SoPlate plate, IReadOnlyList<ItemAmount> inputs, out string failReason);
    
    /// <summary>
    /// Devuelve la cantidad de errores en base a doneness mismatches.
    /// Solo se llama si se valido la identidad.
    /// </summary>
    int EvaluateDonenessMistakes(SoPlate plate, IReadOnlyList<ItemAmount> inputs);

    /// <summary>
    /// Calcula el rating en base a la cantidad de errores.
    /// </summary>
    int ComputeOutputRating(SoPlate plate, int baseRating, int mistakes);
}
