import os
import re


def extract_cs_info(filepath):
    results = []
    in_enum = False

    try:
        with open(filepath, "r", encoding="utf-8-sig") as f:
            lines = f.readlines()

        for line in lines:
            line_stripped = line.strip()

            # Ignorăm comentariile
            if line_stripped.startswith("//"):
                continue

            # Dacă ne aflăm în interiorul unui enum, extragem valorile
            if in_enum:
                if "}" in line_stripped:
                    in_enum = False  # Am ieșit din enum
                else:
                    # Curățăm virgulele și ignorăm liniile care conțin doar acolada de deschidere
                    clean_enum_member = line_stripped.replace(",", "").strip()
                    if clean_enum_member and clean_enum_member != "{":
                        results.append(f"    {clean_enum_member}")
                continue

            # 1. Căutăm definiții de clase, interfețe, enum-uri, struct-uri, record-uri
            class_match = re.search(
                r"^(?:public|private|protected|internal)?\s*(?:static|sealed|abstract|partial)?\s*(class|enum|interface|record|struct)\s+(\w+)",
                line_stripped,
            )
            if class_match:
                type_keyword = class_match.group(1)
                type_name = class_match.group(2)
                results.append(f"{type_keyword} {type_name}:")

                # Activăm starea de citire totală dacă este enum
                if type_keyword == "enum":
                    # Verificăm dacă enum-ul este scris pe o singură linie (ex: public enum Role { Admin, User })
                    if "{" in line_stripped and "}" in line_stripped:
                        content = line_stripped[
                            line_stripped.find("{") + 1 : line_stripped.find("}")
                        ]
                        members = [m.strip() for m in content.split(",") if m.strip()]
                        for m in members:
                            results.append(f"    {m}")
                    else:
                        in_enum = True
                continue

            # 2. Căutăm proprietăți și semnături de funcții pentru clase/interfețe
            member_match = re.match(
                r"^(public|private|protected|internal)\s+(.*)", line_stripped
            )
            if member_match:
                # Curățăm implementarea (ștergem tot ce e după {, => sau ;)
                clean_member = (
                    line_stripped.split("{")[0].split("=>")[0].split(";")[0].strip()
                )
                if (
                    clean_member
                    and not clean_member.startswith("return")
                    and not clean_member.startswith("throw")
                ):
                    results.append(f"    {clean_member}")

    except Exception as e:
        results.append(f"    [Eroare la citirea fișierului: {str(e)}]")

    return results


def extract_cshtml_info(filepath):
    results = []
    try:
        with open(filepath, "r", encoding="utf-8-sig") as f:
            for line in f:
                line_stripped = line.strip()
                # Extragem doar liniile care folosesc Tag Helpers (asp-)
                if "asp-" in line_stripped:
                    results.append(f"    {line_stripped}")
    except Exception as e:
        results.append(f"    [Eroare la citirea fișierului: {str(e)}]")
    return results


def process_solution(root_dir):
    projects = {}
    for root, dirs, files in os.walk(root_dir):
        dirs[:] = [
            d
            for d in dirs
            if d not in ["bin", "obj", "node_modules", ".git", ".vs", "Migrations"]
        ]

        for file in files:
            if file.endswith(".csproj"):
                project_name = file.replace(".csproj", "")
                projects[project_name] = root

    if not projects:
        projects["CinemaSystem_Context"] = root_dir

    for proj_name, proj_path in projects.items():
        output_lines = []

        for root, dirs, files in os.walk(proj_path):
            dirs[:] = [
                d
                for d in dirs
                if d not in ["bin", "obj", "node_modules", ".git", ".vs", "Migrations"]
            ]

            for file in files:
                filepath = os.path.join(root, file)
                rel_path = os.path.relpath(filepath, proj_path).replace("\\", "/")

                if file.endswith(".cs"):
                    info = extract_cs_info(filepath)
                    if info:
                        output_lines.append(f"- {rel_path}")
                        output_lines.extend(info)
                        output_lines.append("")

                elif file.endswith(".cshtml"):
                    info = extract_cshtml_info(filepath)
                    if info:
                        output_lines.append(f"- {rel_path}")
                        output_lines.extend(info)
                        output_lines.append("")

        if output_lines:
            out_file = f"{proj_name}.md"
            with open(out_file, "w", encoding="utf-8") as f:
                f.write("\n".join(output_lines))
            print(f"Generat cu succes: {out_file}")


if __name__ == "__main__":
    print("Începere scanare soluție...")
    process_solution(".")
    print("Scanare finalizată!")
