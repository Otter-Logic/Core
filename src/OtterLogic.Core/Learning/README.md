# Learning

Runtime inference and dataset capture. Training does **not** happen here — see
`/python`.

The intended split:

- `DatasetWriter` — records generated designs (inputs + outputs) for training.
- `OnnxSurrogate` — loads a `.onnx` from `/models` and evaluates it.

To add inference, reference the ONNX runtime from *this* project only:

```
dotnet add src/OtterLogic.Core package Microsoft.ML.OnnxRuntime
```

Note it carries a native dependency, so leave `ExcludeAssets="runtime"` off that
one — it genuinely does need to be copied next to the `.gha`.
