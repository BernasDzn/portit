#!/usr/bin/env python3
"""
Generate valid ISO 6346 container numbers (format: ABCD1234560)
- 4 letters: owner code (3 letters) + equipment category identifier (usually 'U')
- 6 digits: serial number
- 1 digit: check digit (ISO 6346 algorithm)

Usage:
    python gen_containers.py
"""

import random
import string

# Letter to numeric mapping per ISO 6346 (A=10, B=12, C=13, ... with 11,22,33 omitted)
LETTER_VALUES = {
    'A': 10, 'B': 12, 'C': 13, 'D': 14, 'E': 15, 'F': 16, 'G': 17,
    'H': 18, 'I': 19, 'J': 20, 'K': 21, 'L': 23, 'M': 24,
    'N': 25, 'O': 26, 'P': 27, 'Q': 28, 'R': 29, 'S': 30,
    'T': 31, 'U': 32, 'V': 34, 'W': 35, 'X': 36, 'Y': 37, 'Z': 38
}

# Precomputed positional factors: 2^position for positions 0..9 (11 characters total,
# but only first 10 are used for the multiplication; the 11th is the check digit itself)
POSITION_FACTORS = [2 ** i for i in range(10)]  # [1,2,4,8,16,32,64,128,256,512]


def compute_check_digit(prefix4: str, serial6: str) -> int:
    """
    Compute ISO 6346 check digit for:
      - prefix4: 4-letter code (owner 3 letters + category letter)
      - serial6: 6-digit serial number (string)
    Returns integer 0..9
    """
    if len(prefix4) != 4 or len(serial6) != 6:
        raise ValueError("prefix4 must be 4 letters and serial6 must be 6 digits")

    s = 0
    # iterate over first 10 characters: 4 letters + 6 digits
    combined = (prefix4 + serial6).upper()
    for pos, ch in enumerate(combined):
        # convert char to numeric value
        if ch.isalpha():
            val = LETTER_VALUES.get(ch)
            if val is None:
                raise ValueError(f"Invalid letter in prefix: {ch}")
        else:
            # digit
            val = int(ch)
        factor = POSITION_FACTORS[pos]  # 2^pos
        s += val * factor

    remainder = s % 11
    # if remainder == 10 -> check digit is 0 (the standard recommends avoiding serials that give 10)
    check_digit = remainder if remainder != 10 else 0
    return check_digit


def random_owner_code(registered_only: bool = False) -> str:
    """
    Generate a 4-letter owner+category code.
    By default, we pick:
      - 3 random letters for owner
      - 4th letter 'U' (most common: freight containers)
    If registered_only True, you would need a BIC registry to pick real owner codes (not implemented).
    """
    owner = ''.join(random.choice(string.ascii_uppercase) for _ in range(3))
    category = 'U'  # typical equipment category for freight containers
    return owner + category


def random_serial() -> str:
    """Generate a 6-digit serial number as zero-padded string."""
    return f"{random.randint(0, 999999):06d}"


def generate_container_number(registered_only: bool = False) -> str:
    """
    Generate one valid container number: 4 letters + 6 digits + 1 check digit
    e.g. ABCU1234567
    """
    prefix4 = random_owner_code(registered_only=registered_only)
    serial6 = random_serial()
    cd = compute_check_digit(prefix4, serial6)
    return f"{prefix4}{serial6}{cd}"


if __name__ == "__main__":
    # Example: generate 10 container numbers
    for _ in range(10):
        print(generate_container_number())