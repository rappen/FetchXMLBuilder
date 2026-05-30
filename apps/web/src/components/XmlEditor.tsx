import Editor, { type OnMount } from "@monaco-editor/react";
import { useCallback, useEffect, useRef } from "react";

interface XmlEditorProps {
  value: string;
  onChange: (value: string) => void;
  onCursorOffsetChange?: (offset: number) => void;
}

interface Disposable {
  dispose: () => void;
}

export function XmlEditor({
  value,
  onChange,
  onCursorOffsetChange,
}: XmlEditorProps) {
  const cursorDisposableRef = useRef<Disposable | null>(null);

  useEffect(
    () => () => {
      cursorDisposableRef.current?.dispose();
    },
    [],
  );

  const handleMount = useCallback<OnMount>(
    (editor) => {
      cursorDisposableRef.current?.dispose();
      if (!onCursorOffsetChange) return;

      const emitCursorOffset = () => {
        const model = editor.getModel();
        const position = editor.getPosition();
        if (!model || !position) return;
        onCursorOffsetChange(model.getOffsetAt(position));
      };

      emitCursorOffset();
      cursorDisposableRef.current =
        editor.onDidChangeCursorPosition(emitCursorOffset);
    },
    [onCursorOffsetChange],
  );

  return (
    <Editor
      height="100%"
      language="xml"
      theme="vs"
      value={value}
      onMount={handleMount}
      onChange={(nextValue) => onChange(nextValue ?? "")}
      options={{
        minimap: { enabled: false },
        fontSize: 13,
        lineNumbersMinChars: 3,
        scrollBeyondLastLine: false,
        tabSize: 2,
        wordWrap: "on",
        automaticLayout: true,
      }}
    />
  );
}
