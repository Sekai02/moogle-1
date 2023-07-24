#! /bin/bash

function check_file() {
    if ! test -f $1; then
        MISSING_FILE=1
        echo "[error] The file $1 is missing"
    fi
}

function gen_slides() {
    MISSING_FILE=0
    check_file "moogle.png"
    check_file "presentation.tex"
    if [ $MISSING_FILE -eq 1 ]; then
        echo "[error] Can not compile the slides because some files are missing"
        echo "[suggest] Make sure you include them in the presentación folder and try again"
    else
        pdflatex presentation.tex
        pdflatex presentation.tex
    fi
}

function gen_report() {
    MISSING_FILE=0
    check_file "report.tex"
    if [ $MISSING_FILE -eq 1 ]; then
        echo "[error] Can not compile the slides because some files are missing"
        echo "[suggest] Make sure you include them in the presentación folder and try again"
    else
        pdflatex report.tex
        pdflatex report.tex
    fi
}

if [ $# -lt 1 ]; then
    echo "Script options:"
    echo "run : Runs the Moogle project"
    echo "report: Compiles and generates report.pdf"
    echo "slides: Compiles and generates presentation.pdf"
    echo "show_report <viewer>: Opens report.pdf with the specified pdf viewer or the default one if not specified (if report.pdf exists and it generates it if it does not)"
    echo "show_slides <viewer>: Opens presentation.pdf with the specified pdf viewer or the default one if not specified (if presentation.pdf exists and it generates it if it does not)"
    echo "clean: Removes unnecessary files generated while compiling report.pdf and presentation.pdf"
elif [ "$1" = "run" ]; then
    cd ..
    dotnet watch run --project MoogleServer
elif [ "$1" = "report" ]; then
    cd ../informe
    gen_report
elif [ "$1" = "slides" ]; then
    cd ../presentación
    gen_slides
elif [ "$1" = "show_report" ]; then
    cd ../informe

    if ! test -f report.pdf; then
        gen_report
    fi

    if [ $# -gt 1 ]; then
        "$2" report.pdf
    else
        open report.pdf
    fi
elif [ "$1" = "show_slides" ]; then
    cd ../presentación

    if ! test -f presentation.pdf; then
        gen_slides
    fi

    if [ $# -gt 1 ]; then
        "$2" presentation.pdf
    else
        open presentation.pdf
    fi
elif [ "$1" = "clean" ]; then
    GLOBIGNORE=report.pdf:report.tex:presentation.pdf:presentation.tex:moogle.png
    cd ../informe
    rm -v *
    cd ../presentación
    rm -v *
else
    echo "Invalid command"
fi
