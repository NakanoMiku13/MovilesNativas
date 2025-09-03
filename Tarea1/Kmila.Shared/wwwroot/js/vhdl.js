// VHDL Language Definition for Monaco Editor
// This script should be included after Monaco Editor is loaded

// Register VHDL language
monaco.languages.register({ id: 'vhdl' });

// Define VHDL language syntax highlighting
monaco.languages.setMonarchTokensProvider('vhdl', {
    // Set defaultToken to invalid to see what tokens are not matched.
    defaultToken: 'invalid',
    
    // Keywords
    keywords: [
        'abs', 'access', 'after', 'alias', 'all', 'and', 'architecture', 'array',
        'assert', 'attribute', 'begin', 'block', 'body', 'buffer', 'bus', 'case',
        'component', 'configuration', 'constant', 'disconnect', 'downto', 'else',
        'elsif', 'end', 'entity', 'exit', 'file', 'for', 'function', 'generate',
        'generic', 'group', 'guarded', 'if', 'impure', 'in', 'inertial', 'inout',
        'is', 'label', 'library', 'linkage', 'literal', 'loop', 'map', 'mod',
        'nand', 'new', 'next', 'nor', 'not', 'null', 'of', 'on', 'open', 'or',
        'others', 'out', 'package', 'port', 'postponed', 'procedure', 'process',
        'pure', 'range', 'record', 'register', 'reject', 'rem', 'report', 'return',
        'rol', 'ror', 'select', 'severity', 'signal', 'shared', 'sla', 'sli',
        'sra', 'srl', 'subtype', 'then', 'to', 'transport', 'type', 'unaffected',
        'units', 'until', 'use', 'variable', 'wait', 'when', 'while', 'with',
        'xnor', 'xor'
    ],

    // Standard types
    types: [
        'bit', 'bit_vector', 'boolean', 'character', 'integer', 'natural',
        'positive', 'real', 'string', 'time', 'std_logic', 'std_logic_vector',
        'std_ulogic', 'std_ulogic_vector', 'signed', 'unsigned', 'line', 'text',
        'side', 'width', 'severity_level'
    ],

    // Standard functions and procedures
    builtins: [
        'rising_edge', 'falling_edge', 'now', 'readline', 'writeline', 'endline',
        'to_integer', 'to_unsigned', 'to_signed', 'conv_integer', 'conv_std_logic_vector',
        'resize', 'shift_left', 'shift_right', 'rotate_left', 'rotate_right'
    ],

    // Operators
    operators: [
        '=', '/=', '<', '<=', '>', '>=', '+', '-', '*', '/', '**', 'mod', 'rem',
        'and', 'or', 'nand', 'nor', 'xor', 'xnor', 'not', '&', 'sll', 'srl',
        'sla', 'sra', 'rol', 'ror'
    ],

    // Common symbols
    symbols: /[=><!~?:&|+\-*\/\^%]+/,

    // Escape sequences
    escapes: /\\(?:[abfnrtv\\"']|x[0-9A-Fa-f]{1,4}|u[0-9A-Fa-f]{4}|U[0-9A-Fa-f]{8})/,

    // Tokenizer rules
    tokenizer: {
        root: [
            // Comments
            [/--.*$/, 'comment'],
            
            // Multi-line comments (VHDL 2008)
            [/\/\*/, 'comment', '@comment'],

            // Identifiers and keywords
            [/[a-zA-Z_][a-zA-Z0-9_]*/, { 
                cases: { 
                    '@keywords': 'keyword',
                    '@types': 'type',
                    '@builtins': 'predefined',
                    '@default': 'identifier' 
                } 
            }],

            // Numbers
            [/\d+#[0-9a-fA-F]+#/, 'number'], // Based literals (e.g., 16#FF#)
            [/\d+\.\d+([eE][\-+]?\d+)?/, 'number.float'], // Real numbers
            [/\d+[eE][\-+]?\d+/, 'number.float'], // Scientific notation
            [/\d+/, 'number'],

            // Strings
            [/"([^"\\]|\\.)*$/, 'string.invalid'], // Unterminated string
            [/"/, 'string', '@string'],

            // Character literals
            [/'[^\\']'/, 'string'],
            [/(')(@escapes)(')/, ['string', 'string.escape', 'string']],
            [/'/, 'string.invalid'],

            // Delimiters and operators
            [/[{}()\[\]]/, '@brackets'],
            [/[<>](?!@symbols)/, '@brackets'],
            [/@symbols/, {
                cases: {
                    '@operators': 'operator',
                    '@default': ''
                }
            }],

            // Whitespace
            { include: '@whitespace' },

            // Delimiters
            [/[;,.]/, 'delimiter'],
        ],

        comment: [
            [/[^\/*]+/, 'comment'],
            [/\/\*/, 'comment', '@push'],    // Nested comment
            ["\\*/", 'comment', '@pop'],
            [/[\/*]/, 'comment']
        ],

        string: [
            [/[^\\"]+/, 'string'],
            [/@escapes/, 'string.escape'],
            [/\\./, 'string.escape.invalid'],
            [/"/, 'string', '@pop']
        ],

        whitespace: [
            [/[ \t\r\n]+/, 'white'],
            [/\/\*/, 'comment', '@comment'],
            [/--.*$/, 'comment'],
        ],
    },
});

// Define language configuration
monaco.languages.setLanguageConfiguration('vhdl', {
    comments: {
        lineComment: '--',
        blockComment: ['/*', '*/']
    },
    brackets: [
        ['{', '}'],
        ['[', ']'],
        ['(', ')']
    ],
    autoClosingPairs: [
        { open: '{', close: '}' },
        { open: '[', close: ']' },
        { open: '(', close: ')' },
        { open: '"', close: '"', notIn: ['string'] },
        { open: "'", close: "'", notIn: ['string', 'comment'] }
    ],
    surroundingPairs: [
        { open: '{', close: '}' },
        { open: '[', close: ']' },
        { open: '(', close: ')' },
        { open: '"', close: '"' },
        { open: "'", close: "'" }
    ],
    folding: {
        markers: {
            start: /^\s*(entity|architecture|process|if|for|while|case)\b/i,
            end: /^\s*end\b/i
        }
    },
    indentationRules: {
        increaseIndentPattern: /^\s*(entity|architecture|process|if|for|while|case|begin)\b.*$/i,
        decreaseIndentPattern: /^\s*(end|else|elsif|when)\b.*$/i
    }
});

// Optional: Add basic completion provider
monaco.languages.registerCompletionItemProvider('vhdl', {
    provideCompletionItems: function(model, position) {
        // Get the word being typed
        const word = model.getWordUntilPosition(position);
        const range = {
            startLineNumber: position.lineNumber,
            endLineNumber: position.lineNumber,
            startColumn: word.startColumn,
            endColumn: word.endColumn
        };

        const suggestions = [
            // Keywords
            ...['entity', 'architecture', 'signal', 'process', 'begin', 'end', 'if', 'then', 'else', 'elsif'].map(keyword => ({
                label: keyword,
                kind: monaco.languages.CompletionItemKind.Keyword,
                insertText: keyword,
                range: range
            })),
            // Types
            ...['std_logic', 'std_logic_vector', 'integer', 'boolean'].map(type => ({
                label: type,
                kind: monaco.languages.CompletionItemKind.Class,
                insertText: type,
                range: range
            })),
            // Common snippets
            {
                label: 'process',
                kind: monaco.languages.CompletionItemKind.Snippet,
                insertText: [
                    'process(${1:clk})',
                    'begin',
                    '\t$0',
                    'end process;'
                ].join('\n'),
                insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                range: range
            },
            {
                label: 'entity',
                kind: monaco.languages.CompletionItemKind.Snippet,
                insertText: [
                    'entity ${1:entity_name} is',
                    '\tport(',
                    '\t\t${2:port_name} : ${3:in} ${4:std_logic}',
                    '\t);',
                    'end entity ${1:entity_name};'
                ].join('\n'),
                insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                range: range
            }
        ];

        return { suggestions: suggestions };
    }
});