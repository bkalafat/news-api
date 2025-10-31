import typescriptParser from "@typescript-eslint/parser";
import unusedImportsPlugin from "eslint-plugin-unused-imports";
import nextPlugin from "@next/eslint-plugin-next";

export default [
  {
    ignores: [
      "**/node_modules/**",
      "**/.next/**",
      "**/out/**",
      "**/build/**",
      "**/dist/**",
      "**/coverage/**",
    ],
  },
  {
    files: ["**/*.{js,jsx,ts,tsx,mjs}"],
    plugins: {
      "@next/next": nextPlugin,
      "unused-imports": unusedImportsPlugin,
    },
    languageOptions: {
      parser: typescriptParser,
      parserOptions: {
        ecmaVersion: 2021,
        sourceType: "module",
      },
    },
    rules: {
      // Next.js rules
      "@next/next/no-html-link-for-pages": "off",
      
      // Unused code detection
      "no-unused-vars": "off",
      "unused-imports/no-unused-imports": "error",
      "unused-imports/no-unused-vars": [
        "warn",
        {
          vars: "all",
          varsIgnorePattern: "^_",
          args: "after-used",
          argsIgnorePattern: "^_",
        },
      ],

      // Code quality
      "no-console": ["warn", { allow: ["warn", "error"] }],
      "no-debugger": "error",
      "no-var": "error",
      "prefer-const": "error",
      "prefer-template": "warn",
      "no-useless-return": "warn",
      "no-lonely-if": "warn",
      "no-else-return": "warn",
      "object-shorthand": ["warn", "always"],
    },
  },
];
