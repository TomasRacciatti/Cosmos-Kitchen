Para usar droppealo al Outline.cs en el objeto, cualquier cosa que tenga como hijo tmb lo va a recibir *importante*
puede servir si esta bien configurado para tener varias cosas debajo de un solo padre con el code
suponiendo q todas funcionen al mismo tiempo, sino hagan directo al obj

el outline se carga en el play, la opcion de 'precompute' es unicamenta 
para q no sea tan pesado en el start por si hay muchas cosas o tienen mucha complejidad los modelos 

se supone q se puede agregar con codigo directamente con esto

    var outline = gameObject.AddComponent<Outline>();

    outline.OutlineMode = Outline.Mode.OutlineAll;
    outline.OutlineColor = Color.yellow;
    outline.OutlineWidth = 5f;

se puede usar esta linea para prender/apagar el outline

    outline.enabled

se rompe mucho si se saca y se agrega constantemente asi q traten de usar eso


Si salen errores/se descentra prueben con esto

1- setten 'Read/Write Enabled' en el setting de los imports de modelos en el que los usen
2- desactiven 'Optimize Mesh Data' en los player settings
