#include "rot13/kata.hpp"

#include <sstream>

namespace rot13
{
    std::string rot13(const std::string& msg)
    {
        auto result = std::ostringstream();
        for (const auto& character : msg)
        {
            if (character >= 'A' && character <= 'Z')
            {
                result << static_cast<char>((character - 'A' + 13) % 26 + 'A');
            }
            else if (character >= 'a' && character <= 'z')
            {
                result << static_cast<char>((character - 'a' + 13) % 26 + 'a');
            }
            else
            {
                result << character;
            }
        }

        return result.str();
    }
}
