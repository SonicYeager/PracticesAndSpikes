import { assert } from 'chai';
import solution from './index.js';

describe("Split Strings", () => {
    it("Basic tests", () => {
        assert.deepEqual(solution("abcdef"), ["ab", "cd", "ef"]);
        assert.deepEqual(solution("abcdefg"), ["ab", "cd", "ef", "g_"]);
        assert.deepEqual(solution(""), []);
    });
});