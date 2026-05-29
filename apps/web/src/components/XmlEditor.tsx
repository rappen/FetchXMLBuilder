import Editor from "@monaco-editor/react";

interface XmlEditorProps {
  value: string;
  onChange: (value: string) => void;
}

export function XmlEditor({ value, onChange }: XmlEditorProps) {
  return (
    <Editor
      height="100%"
      language="xml"
      theme="vs"
      value={value}
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
