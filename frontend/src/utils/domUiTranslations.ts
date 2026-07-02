import { trLoose } from '../i18n';

const SKIP_TAGS = new Set(['SCRIPT', 'STYLE', 'TEXTAREA']);
const ATTRIBUTES = ['placeholder', 'title', 'aria-label'] as const;

function translateText(value: string) {
  const trimmed = value.trim();
  if (!trimmed) return value;
  const translated = trLoose(trimmed);
  return translated === trimmed ? value : value.replace(trimmed, translated);
}

function translateElement(element: Element) {
  if (SKIP_TAGS.has(element.tagName)) return;
  ATTRIBUTES.forEach((attribute) => {
    const value = element.getAttribute(attribute);
    if (!value) return;
    const translated = translateText(value);
    if (translated !== value) element.setAttribute(attribute, translated);
  });
}

function translateNode(node: Node) {
  if (node.nodeType === Node.TEXT_NODE) {
    const parent = node.parentElement;
    if (!parent || SKIP_TAGS.has(parent.tagName)) return;
    const translated = translateText(node.textContent ?? '');
    if (translated !== node.textContent) node.textContent = translated;
    return;
  }

  if (node.nodeType !== Node.ELEMENT_NODE) return;
  const element = node as Element;
  translateElement(element);
  element.querySelectorAll('*').forEach(translateElement);
  const walker = document.createTreeWalker(element, NodeFilter.SHOW_TEXT);
  const textNodes: Node[] = [];
  while (walker.nextNode()) textNodes.push(walker.currentNode);
  textNodes.forEach(translateNode);
}

export function translateStaticUi(root: ParentNode = document.body) {
  root.childNodes.forEach(translateNode);
}

export function installStaticUiTranslator() {
  if (typeof document === 'undefined') return () => undefined;
  translateStaticUi();
  const observer = new MutationObserver((mutations) => {
    mutations.forEach((mutation) => {
      if (mutation.type === 'attributes') {
        translateElement(mutation.target as Element);
        return;
      }
      mutation.addedNodes.forEach(translateNode);
      if (mutation.type === 'characterData') translateNode(mutation.target);
    });
  });
  observer.observe(document.body, {
    attributes: true,
    attributeFilter: [...ATTRIBUTES],
    childList: true,
    characterData: true,
    subtree: true,
  });
  return () => observer.disconnect();
}
